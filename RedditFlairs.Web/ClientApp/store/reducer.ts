import { ActionType as CreateActionType } from 'typesafe-actions'

import * as actions from './actions'

export type ActionType = CreateActionType<typeof actions>

export default function reducer(
    state: Readonly<app.IState> = {},
    action: ActionType
): Readonly<app.IState> {
    switch (action.type) {
    case 'ADD_SUMMONER':
        return {
            ...state,
            profile: {
                ...state.profile,
                summoners: [
                    ...state.profile.summoners,
                    action.payload
                ]
            }
        }
    
    case 'REMOVE_SUMMONER':
        return {
            ...state,
            profile: {
                ...state.profile,
                summoners: state.profile.summoners.filter(x => x.id !== action.payload)
            }
        }

    case 'SET_LOADED':
        return {
            ...state,
            loaded: action.payload
        }
        
    case 'SET_MODAL':
        return {
            ...state,
            modal: action.payload
        }

    case 'SET_PROFILE':
        return {
            ...state,
            profile: action.payload
        }
    default:
        return { ...state }
    }
}
