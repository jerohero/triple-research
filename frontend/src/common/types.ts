export interface Project {
  Id: number,
  Name: string,
  VisionSets: object[],
  TrainedModels: object[],
}

export interface VisionSet {
  Id: number,
  ProjectId: number,
  Name: string,
  Sources: string[]
}

export interface Session {
  Id: number,
  VisionSetId: number,
  IsActive: boolean,
  Source: string,
  Pod: string,
  CreatedAt: Date,
  StartedAt: Date,
  StoppedAt: Date
}

export interface TrainedModel {
  Id: number,
  ProjectId: number,
  Name: string,
  IsUploadFinished: boolean
}

export interface Negotiate {
  Url: string
}
