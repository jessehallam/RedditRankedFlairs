import * as redux from 'redux'

import * as actions from './actions'
import defaultReducer, { ActionType } from './reducer'

export { actions }
export { ActionType }
export type Dispatch = redux.Dispatch<ActionType>

export default redux.createStore(defaultReducer,
    {},
    window.__REDUX_DEVTOOLS_EXTENSION__ && window.__REDUX_DEVTOOLS_EXTENSION__())