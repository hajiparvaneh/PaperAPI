export interface PdfOptions {
  pageSize?: string;
  orientation?: string;
  marginTop?: number;
  marginRight?: number;
  marginBottom?: number;
  marginLeft?: number;
  printMediaType?: boolean;
  disableSmartShrinking?: boolean;
  enableJavascript?: boolean;
  disableJavascript?: boolean;
  headerLeft?: string;
  headerCenter?: string;
  headerRight?: string;
  footerLeft?: string;
  footerCenter?: string;
  footerRight?: string;
  headerSpacing?: number;
  footerSpacing?: number;
  headerHtml?: string;
  footerHtml?: string;
  dpi?: number;
  zoom?: number;
  imageDpi?: number;
  imageQuality?: number;
  lowQuality?: boolean;
  images?: boolean;
  noImages?: boolean;
}

export interface PdfGenerateRequest {
  html: string;
  options?: PdfOptions;
}

export interface PaperApiClientOptions {
  /**
   * PaperAPI API key. Required.
   */
  apiKey: string;
  /**
   * Overrides the base URL (defaults to https://api.paperapi.de/).
   */
  baseUrl?: string;
  /**
   * Custom fetch implementation (node-fetch, undici, etc.).
   */
  fetch?: typeof fetch;
  /**
   * Custom user agent identifier. Only sent in Node runtimes.
   */
  userAgent?: string;
}

export interface PaperApiRequestOptions {
  /**
   * Optional AbortSignal passed to fetch.
   */
  signal?: AbortSignal;
}
