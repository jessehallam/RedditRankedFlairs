import * as React from 'react'
import { hot } from 'react-hot-loader'
import { connect } from 'react-redux'

import Modal from './modal'
import NavBar from './NavBar'
import SignIn from './SignIn'
import SummonerList from './summoner/SummonerList'

interface IStoreProps {
    loaded: boolean,
    profile: app.IProfile
}

interface IAppProps extends IStoreProps {}

class App extends React.Component<IAppProps> {
    render() {
        if (!this.props.loaded) return null
        return (
            <div className='app-root'>
                <NavBar />
                <Modal />
                <div className='content-main'>
                    {!this.props.profile && <SignIn />}
                    {this.props.profile && <SummonerList />}
                </div>
            </div>
        )
    }

    static mapProps = (state: app.IState): IStoreProps => ({
        loaded: state.loaded,
        profile: state.profile
    })
}

const ConnectedApp = connect(App.mapProps)(App)
const HotApp = hot(module)(ConnectedApp)

export default HotApp