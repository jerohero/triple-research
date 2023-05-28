export interface Project {
  Id: number,
  Name: string,
  VisionSets: object[],
  TrainedModels: object[],
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

export interface Negotiate {
  Url: string
}
