import * as React from 'react'
import * as ReactDOM from 'react-dom'
import * as ReactRedux from 'react-redux'

import './styles/main.scss'

import App from './components/App'
import store from './store'

import boot from './boot'
boot()

const mountPoint = document.createElement('div')
document.body.appendChild(mountPoint)

ReactDOM.render(
    <ReactRedux.Provider store={store}>
        <App />
    </ReactRedux.Provider>,
    mountPoint
)