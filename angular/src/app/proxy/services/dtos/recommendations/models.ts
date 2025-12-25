import type { FullAuditedEntityDto } from '@abp/ng.core';
import type { RecommendationType } from '../../../entities/recommendations/recommendation-type.enum';

export interface RecommendationDto extends FullAuditedEntityDto<string> {
  userId?: string;
  type?: RecommendationType;
  title?: string;
  externalUrl?: string;
  imageUrl?: string;
  reasoning?: string;
}
