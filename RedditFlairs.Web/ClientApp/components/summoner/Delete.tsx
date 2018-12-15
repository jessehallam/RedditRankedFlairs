import * as React from 'react'
import { IRenderProps } from '../modal'

interface IDeleteProps extends IRenderProps {
    summoner: app.ISummoner
}

export class Delete extends React.Component<IDeleteProps> {
    render() {
        const { summoner } = this.props
        return (
            <div className='modal-body'>
                <p><strong>Delete Summoner?</strong></p>
                <p>
                    Are you sure you want to delete <strong>{summoner.summonerName} ({summoner.region})</strong>?
                </p>
                <div className='text-right'>
                    <button className='btn btn-sm btn-danger' onClick={this.onConfirm}>
                        Delete
                    </button>

                    <button className='btn btn-default btn-sm ml-1' onClick={this.onCancel}>
                        Cancel
                    </button>
                </div>
            </div>
        )
    }

    private onCancel = () => this.props.cancel()
    private onConfirm = () => this.props.confirm(true)
}

export default Delete