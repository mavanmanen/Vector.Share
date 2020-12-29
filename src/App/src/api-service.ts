import axios, { AxiosInstance, AxiosRequestConfig } from "axios"
import { eventBus } from "./main"
import { UploadModel, UploadResultModel } from "./models"

export default class ApiService {
    private _axios: AxiosInstance

    constructor() {
        this._axios = axios.create({
            baseURL: process.env.VUE_APP_API_URL
        })
    }

    public async uploadAsync(model: UploadModel): Promise<UploadResultModel | null> {
        const formData = new FormData()
        formData.append("filedata", model.file as Blob)
        formData.append("lifetime", model.lifetime.toString())

        const config: AxiosRequestConfig = {
            headers: {
                'Content-Type': 'multipart/form-data'
            },
            onUploadProgress: event => {
                const percentCompleted = Math.floor((event.loaded * 100) / event.total)
                eventBus.emit('onUploadProgress', percentCompleted)
            }
        }

        const result = await this._axios.post<UploadResultModel>('upload', formData, config)
        return result.status == 200 ? result.data : null
    }
}