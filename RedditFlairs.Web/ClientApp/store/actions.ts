import { action } from 'typesafe-actions'

export const addSummoner = (summoner: app.ISummoner) => action('ADD_SUMMONER', summoner)

export const removeSummoner = (id: number) => action('REMOVE_SUMMONER', id)

export const setLoaded = () => action('SET_LOADED', true)
export const setModal = (name: string | 'none') => action('SET_MODAL', name)
export const setProfile = (profile: app.IProfile) => action('SET_PROFILE', profile)