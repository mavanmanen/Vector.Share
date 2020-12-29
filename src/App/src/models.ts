export enum FileLifetime {
    D1,
    D7,
    M1
}

export interface UploadModel {
    lifetime: FileLifetime;
    file: Blob | null;
}

export interface UploadResultModel {
    url: string;
}