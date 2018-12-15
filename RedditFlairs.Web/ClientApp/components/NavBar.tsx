import * as React from 'react'
import { connect } from 'react-redux'

import { show } from './modal'
import Register from './Register'

interface IStoreProps {
    authenticated: boolean
}

interface INavBarProps extends IStoreProps {}

interface INavBarState {
    showRegister?: boolean
}

class NavBar extends React.Component<INavBarProps> {
    readonly state: INavBarState = {}

    render() {
        return (
            <nav className='navbar navbar-expand navbar-dark bg-dark'>
                <div className='collapse navbar-collapse'>
                    <a className='navbar-brand' href='#'>
                        Ranked Reddit Flairs
                    </a>
                    <ul className='navbar-nav mr-auto'>
                        {this.props.authenticated && <RegisterButton />}
                    </ul>
                </div>
            </nav>
        )
    }

    static mapProps = (state: app.IState): IStoreProps => ({
        authenticated: !!state.profile
    })
}

function RegisterButton() {
    const onClick = () => show((props) => <Register {...props} />)
    return (
        <button className='btn btn-success' onClick={onClick}>
            Register Summoner
        </button>
    )
}

export default connect(NavBar.mapProps)(NavBar)
