import { mapEnumToOptions } from '@abp/ng.core';

export enum RecommendationType {
  Music = 0,
  Meditation = 1,
  Book = 2,
  Activity = 3,
  Article = 4,
}

export const recommendationTypeOptions = mapEnumToOptions(RecommendationType);
