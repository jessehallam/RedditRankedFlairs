import * as ajax from './util/ajax'

export const deleteSummoner = (id: number) => ajax.post(`/api/register/delete/${id}`, null)
export const getProfile = () => ajax.get<app.IProfile>(`/api/profile`)
export const getRegions = () => ajax.get<string[]>(`/api/register/regions`)
export const register = async (region: string, summonerName: string) =>
    ajax.post<app.ISummoner>(`/api/register`, {
        region,
        summonerName
    })
