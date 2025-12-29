import type { ZodType } from 'zod';
import { PaperApiError } from './errors';
import type { PaperApiClientOptions, PaperApiRequestOptions, PdfGenerateRequest } from './types';
import {
  PdfJobStatusSchema,
  UsageResponseSchema,
  WhoAmIResponseSchema,
  type PdfJobStatusResponse,
  type UsageResponse,
  type WhoAmIResponse
} from './schemas';

const DEFAULT_BASE_URL = 'https://api.paperapi.de/';
const DEFAULT_USER_AGENT = 'PaperApiJsSdk/0.1.0';

const ensureTrailingSlash = (value: string): string => (value.endsWith('/') ? value : `${value}/`);

const isNodeRuntime = (): boolean =>
  typeof process !== 'undefined' && typeof process.release !== 'undefined' && process.release.name === 'node';

export class PaperApiClient {
  private readonly baseUrl: string;
  private readonly apiKey: string;
  private readonly fetchImpl: typeof fetch;
  private readonly userAgent: string;
  private readonly sendUserAgentHeader: boolean;

  constructor(options: PaperApiClientOptions) {
    if (!options || !options.apiKey || !options.apiKey.trim()) {
      throw new Error('PaperApiClient requires a non-empty apiKey.');
    }

    this.baseUrl = ensureTrailingSlash(options.baseUrl ?? DEFAULT_BASE_URL);
    try {
      // Validate eagerly so every request does not have to handle malformed base URLs.
      void new URL(this.baseUrl);
    } catch {
      throw new Error('PaperApiClient baseUrl must be an absolute URL.');
    }
    this.apiKey = options.apiKey.trim();
    this.fetchImpl = options.fetch ?? globalThis.fetch;

    if (typeof this.fetchImpl !== 'function') {
      throw new Error('A fetch implementation is required. Provide options.fetch when targeting Node < 18.');
    }

    this.userAgent = options.userAgent ?? DEFAULT_USER_AGENT;
    this.sendUserAgentHeader = isNodeRuntime();
  }

  async generatePdf(
    request: PdfGenerateRequest,
    options?: PaperApiRequestOptions
  ): Promise<ArrayBuffer> {
    this.ensureValidGenerateRequest(request);
    return this.sendForBinary('POST', 'v1/generate', request, 'application/pdf', options);
  }

  async enqueuePdfJob(
    request: PdfGenerateRequest,
    options?: PaperApiRequestOptions
  ): Promise<PdfJobStatusResponse> {
    this.ensureValidGenerateRequest(request);
    return this.sendForJson('POST', 'v1/generate-async', request, PdfJobStatusSchema, options);
  }

  async getJobStatus(jobId: string, options?: PaperApiRequestOptions): Promise<PdfJobStatusResponse> {
    const id = this.ensureJobId(jobId);
    return this.sendForJson('GET', `v1/jobs/${id}`, undefined, PdfJobStatusSchema, options);
  }

  async downloadJobResult(jobId: string, options?: PaperApiRequestOptions): Promise<ArrayBuffer> {
    const id = this.ensureJobId(jobId);
    return this.sendForBinary('GET', `v1/jobs/${id}/result`, undefined, 'application/pdf', options);
  }

  async getUsageSummary(options?: PaperApiRequestOptions): Promise<UsageResponse> {
    return this.sendForJson('GET', 'v1/usage', undefined, UsageResponseSchema, options);
  }

  async getWhoAmI(options?: PaperApiRequestOptions): Promise<WhoAmIResponse> {
    return this.sendForJson('GET', 'v1/whoami', undefined, WhoAmIResponseSchema, options);
  }

  async checkHealth(options?: PaperApiRequestOptions): Promise<boolean> {
    await this.send('GET', 'health', undefined, 'application/json', options);
    return true;
  }

  private ensureValidGenerateRequest(request: PdfGenerateRequest | undefined): void {
    if (!request || typeof request.html !== 'string' || !request.html.trim()) {
      throw new Error('PdfGenerateRequest.html must be a non-empty string.');
    }
  }

  private ensureJobId(jobId: string): string {
    if (!jobId || typeof jobId !== 'string' || !jobId.trim()) {
      throw new Error('jobId must be a non-empty string.');
    }
    return jobId;
  }

  private buildHeaders(accept?: string, contentType?: string): HeadersInit {
    const headers: Record<string, string> = {
      Authorization: `Bearer ${this.apiKey}`,
      'X-PaperApi-Client': this.userAgent
    };
    if (accept) {
      headers.Accept = accept;
    }
    if (contentType) {
      headers['Content-Type'] = contentType;
    }
    if (this.sendUserAgentHeader) {
      headers['User-Agent'] = this.userAgent;
    }
    return headers;
  }

  private async send(
    method: string,
    path: string,
    body: unknown,
    accept: string,
    options?: PaperApiRequestOptions
  ): Promise<Response> {
    const headers = this.buildHeaders(accept, body !== undefined ? 'application/json' : undefined);
    const init: RequestInit = {
      method,
      headers,
      signal: options?.signal
    };

    if (body !== undefined) {
      init.body = JSON.stringify(body);
    }

    const url = new URL(path, this.baseUrl).toString();
    const response = await this.fetchImpl(url, init);

    if (!response.ok) {
      await this.raiseForError(response, path);
    }

    return response;
  }

  private async sendForJson<T>(
    method: string,
    path: string,
    body: unknown,
    schema: ZodType<T, any, any>,
    options?: PaperApiRequestOptions
  ): Promise<T> {
    const response = await this.send(method, path, body, 'application/json', options);
    const requestId = response.headers.get('x-request-id');
    const raw = await response.text();

    if (!raw.trim()) {
      throw new PaperApiError({
        status: response.status,
        statusText: response.statusText,
        responseBody: raw,
        requestId,
        message: `PaperAPI returned an empty response for ${path}`
      });
    }

    let data: unknown;
    try {
      data = JSON.parse(raw);
    } catch (error) {
      throw new PaperApiError({
        status: response.status,
        statusText: response.statusText,
        responseBody: raw,
        requestId,
        message: `PaperAPI returned invalid JSON for ${path}`,
        cause: error
      });
    }

    try {
      return schema.parse(data);
    } catch (error) {
      throw new PaperApiError({
        status: response.status,
        statusText: response.statusText,
        responseBody: raw,
        requestId,
        message: `PaperAPI returned an unexpected payload for ${path}`,
        cause: error
      });
    }
  }

  private async sendForBinary(
    method: string,
    path: string,
    body: unknown,
    accept: string,
    options?: PaperApiRequestOptions
  ): Promise<ArrayBuffer> {
    const response = await this.send(method, path, body, accept, options);
    return response.arrayBuffer();
  }

  private async raiseForError(response: Response, path: string): Promise<never> {
    const requestId = response.headers.get('x-request-id');
    let responseBody: string | undefined;
    let message: string | undefined;
    let errorCode: string | undefined;

    try {
      responseBody = await response.text();
      if (responseBody) {
        try {
          const payload = JSON.parse(responseBody);
          const candidateMessage = payload?.message ?? payload?.error ?? payload?.title;
          if (typeof candidateMessage === 'string' && candidateMessage.trim()) {
            message = candidateMessage;
          }
          const candidateCode = payload?.errorCode ?? payload?.code;
          if (typeof candidateCode === 'string' && candidateCode.trim()) {
            errorCode = candidateCode;
          }
        } catch {
          // leave parsing errors silent; we'll surface the raw body.
        }
      }
    } catch {
      responseBody = undefined;
    }

    throw new PaperApiError({
      status: response.status,
      statusText: response.statusText,
      responseBody,
      requestId,
      errorCode,
      message:
        message ??
        `PaperAPI request to ${path} failed with status code ${response.status} (${response.statusText})`
    });
  }
}
