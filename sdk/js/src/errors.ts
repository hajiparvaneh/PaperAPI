export interface PaperApiErrorOptions {
  status: number;
  statusText: string;
  errorCode?: string;
  responseBody?: string;
  requestId?: string | null;
  message?: string;
  cause?: unknown;
}

export class PaperApiError extends Error {
  readonly status: number;
  readonly statusText: string;
  readonly errorCode?: string;
  readonly responseBody?: string;
  readonly requestId?: string | null;
  readonly cause?: unknown;

  constructor(options: PaperApiErrorOptions) {
    super(options.message ?? `PaperAPI request failed with status code ${options.status}`);
    this.name = 'PaperApiError';
    this.status = options.status;
    this.statusText = options.statusText;
    this.errorCode = options.errorCode;
    this.responseBody = options.responseBody;
    this.requestId = options.requestId;
    this.cause = options.cause;
  }
}
