import { mapEnumToOptions } from '@abp/ng.core';

export enum ChatSender {
  User = 0,
  Assistant = 1,
}

export const chatSenderOptions = mapEnumToOptions(ChatSender);
