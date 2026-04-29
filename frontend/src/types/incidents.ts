export type IncidentStatus = 'Open' | 'Acknowledged' | 'Resolved';

export interface IncidentCommentResponse {
  authorId: string;
  authorName: string;
  message: string;
  createdAt: string;
}

export interface IncidentResponse {
  id: string;
  monitorId: string;
  status: IncidentStatus;
  startedAt: string;
  acknowledgedAt: string | null;
  resolvedAt: string | null;
  acknowledgedBy: string | null;
  reason: string | null;
  comments: IncidentCommentResponse[];
}

export interface AddCommentRequest {
  message: string;
}

export interface ResolveIncidentRequest {
  comment?: string;
}
