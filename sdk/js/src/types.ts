export interface PdfOptions {
  pageSize?: string;
  orientation?: string;
  marginTop?: number;
  marginRight?: number;
  marginBottom?: number;
  marginLeft?: number;
  printMediaType?: boolean;
  disableSmartShrinking?: boolean;
  /**
   * Enables JavaScript execution. Do not use together with `disableJavascript`.
   * If both are set, `disableJavascript` takes precedence.
   */
  enableJavascript?: boolean;
  /**
   * Disables JavaScript execution. Do not use together with `enableJavascript`.
   * If both are set, this option takes precedence.
   */
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
  /**
   * Controls whether images are rendered in the generated PDF.
   * - true: images are included
   * - false: images are omitted
   *
   * If both `images` and `noImages` are specified, `noImages` takes precedence.
   */
  images?: boolean;
  /**
   * @deprecated Use `images: false` instead.
   *
   * When both `images` and `noImages` are provided, `noImages` takes precedence
   * and images will be disabled.
   */
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
