import { mapEnumToOptions } from '@abp/ng.core';

export enum EntryType {
  Text = 0,
  Voice = 1,
}

export const entryTypeOptions = mapEnumToOptions(EntryType);
