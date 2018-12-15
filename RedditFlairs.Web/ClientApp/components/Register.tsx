import * as React from 'react'
import { CloseButton, IRenderProps } from './modal'

import { getRegions, register } from '../api'
import { success } from '../util/notify'

import store, { actions } from '../store'

interface IRegisterProps extends IRenderProps {}
interface IRegisterState {
    busy?: boolean
    error?: string
    region: string
    regions: string[]
    summonerName: string
}

class Register extends React.Component<IRegisterProps> {
    readonly state: IRegisterState = {
        region: '',
        regions: [],
        summonerName: ''
    }

    async componentDidMount() {
        const regions = await getRegions()
        this.setState({
            region: regions[0],
            regions
        })
    }

    render() {
        const regions = this.state.regions.map(region => (
            <option key={region} value={region}>{region}</option>
        ))
        return (
            <div>
                <div className='modal-header'>
                    <h5 className='modal-title'>Register Summoner</h5>
                    <CloseButton />
                </div>
                <form onSubmit={this.onSubmit}>
                    <div className='modal-body'>
                        <div className='form-group'>
                            <label htmlFor='summonerName'>Summoner Name</label>
                            <input
                                className='form-control'
                                id='summonerName'
                                name='summonerName'
                                type='text'
                                required={true}
                                onChange={this.onSummonerNameChange}
                                value={this.state.summonerName}
                            />
                        </div>
                        <div className=''>
                            <label htmlFor='region'>Region</label>
                            <select
                                className='form-control'
                                id='region'
                                name='region'
                                onChange={this.onRegionChange}
                                value={this.state.region}
                            >
                                {regions}
                            </select>
                        </div>
                    </div>
                    <div className='modal-footer'>
                        {this.state.error && (
                            <span className='text-danger'>
                                {this.state.error}
                            </span>
                        )}
                        <button
                            className='btn btn-primary'
                            type='submit'
                            disabled={!this.canSubmit()}
                        >
                            Register
                        </button>
                    </div>
                </form>
            </div>
        )
    }

    private canSubmit = () => this.isValid() && !this.state.busy

    private isValid = () => this.state.region && this.state.summonerName

    private onRegionChange = (e: React.ChangeEvent<HTMLSelectElement>) =>
        this.setState({ region: e.target.value })

    private onSubmit = async (e: React.FormEvent) => {
        e.preventDefault()

        this.setState({
            busy: true,
            error: ''
        })

        const result = await register(this.state.region, this.state.summonerName)

        if (!result.success) {
            this.setState({
                busy: false,
                error: result.error
            })
        } else {
            this.props.confirm()
            store.dispatch(actions.addSummoner(result.data))
            success('Summoner registered successfully.')
        }
    }

    private onSummonerNameChange = (e: React.ChangeEvent<HTMLInputElement>) =>
        this.setState({ summonerName: e.target.value })
}

export default Register
