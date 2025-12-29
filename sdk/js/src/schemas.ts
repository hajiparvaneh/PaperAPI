import { z } from 'zod';

const isoDateSchema = z.string().transform((value) => {
  const date = new Date(value);
  if (Number.isNaN(date.getTime())) {
    throw new Error(`Invalid date: ${value}`);
  }
  return date;
});

export const PdfJobLinksSchema = z.object({
  self: z.string(),
  result: z.string().nullable().optional()
});

export const PdfJobStatusSchema = z.object({
  id: z.string().uuid(),
  status: z.string(),
  errorMessage: z.string().nullable().optional(),
  downloadUrl: z.string().nullable().optional(),
  jobStatusUrl: z.string().nullable().optional(),
  createdAt: isoDateSchema.optional(),
  expiresAt: isoDateSchema.optional(),
  links: PdfJobLinksSchema
});

export const UsageResponseSchema = z.object({
  used: z.number(),
  monthlyLimit: z.number(),
  remaining: z.number(),
  overage: z.number(),
  nextRechargeAt: isoDateSchema.optional()
});

export const WhoAmIPlanResponseSchema = z.object({
  name: z.string(),
  code: z.string(),
  interval: z.string(),
  monthlyLimit: z.number(),
  priceCents: z.number()
});

export const WhoAmIResponseSchema = z.object({
  id: z.string().uuid(),
  name: z.string(),
  email: z.string().email(),
  plan: WhoAmIPlanResponseSchema
});

export type PdfJobLinks = z.infer<typeof PdfJobLinksSchema>;
export type PdfJobStatusResponse = z.infer<typeof PdfJobStatusSchema>;
export type UsageResponse = z.infer<typeof UsageResponseSchema>;
export type WhoAmIPlanResponse = z.infer<typeof WhoAmIPlanResponseSchema>;
export type WhoAmIResponse = z.infer<typeof WhoAmIResponseSchema>;
