import * as signalR from "@microsoft/signalr";

const hubUrl = import.meta.env.HUB_URL;

export const connection = new signalR.HubConnectionBuilder()
  .withUrl(hubUrl)
  .withAutomaticReconnect()
  .build();
