import axios from 'axios'

export interface IApiResult<TData> {
    data: TData
    error: string
    success: boolean
}

export async function get<T>(url: string): Promise<T> {
    const response = await axios.get(url)
    return response.data
}

export async function post<TResult>(url: string, data: any): Promise<IApiResult<TResult>> {
    const response = await axios.post(url, data)
    return response.data
}