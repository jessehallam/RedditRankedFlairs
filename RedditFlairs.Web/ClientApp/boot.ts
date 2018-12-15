import * as api from './api'
import store, { actions } from './store'

export default async () => {
    const profile = await api.getProfile()
    store.dispatch(actions.setProfile(profile))
    store.dispatch(actions.setLoaded())
}