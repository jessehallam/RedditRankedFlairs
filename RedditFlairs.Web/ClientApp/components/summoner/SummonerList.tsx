import * as React from 'react'
import { connect } from 'react-redux'

import Card from './Card'

interface IStoreProps {
    summoners: app.ISummoner[]
}
interface ISummonerListProps extends IStoreProps {}

class SummonerList extends React.Component<ISummonerListProps> {
    render() {
        return <ul className='summoner-list'>{this.getCards()}</ul>
    }

    private getCards = () =>
        this.props.summoners.map(summoner => (
            <li key={summoner.id}>
                <Card summoner={summoner} />
            </li>
        ))
    
    static mapProps = (state: app.IState): IStoreProps => ({
        summoners: state.profile.summoners
    })
}

export default connect(SummonerList.mapProps)(SummonerList)
