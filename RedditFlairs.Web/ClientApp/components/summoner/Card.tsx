import * as React from 'react'

import { show } from '../modal'

import { deleteSummoner } from '../../api'
import { withTooltip } from '../../util/tooltip'

import store, { actions } from '../../store'

import Delete from './Delete'
import Validate from './Validate'

import { ValidationStatus } from '../../constants'

interface ICardProps {
    summoner: app.ISummoner
}

interface ICardState {
    deleted?: boolean
}

class Card extends React.Component<ICardProps> {
    readonly state: ICardState = {
        deleted: false
    }

    render() {
        const className =
            'card summoner-card ' + (this.state.deleted ? 'deleted' : '')
        return (
            <div className={className}>
                <div className='card-body'>
                    <h5 className='card-title'>
                        {this.props.summoner.summonerName}
                    </h5>
                    <h6 className='card-subtitle text-muted mb-2'>
                        {this.props.summoner.region}
                    </h6>
                    <div>
                        <TrashButton onClick={this.onDelete} />
                        {this.getValidation()}
                    </div>
                </div>
            </div>
        )
    }

    private getValidation = () => {
        const status = this.props.summoner.validation.status
        if (status === ValidationStatus.NotValid) {
            const Component = this.getValidationRequired()
            return <Component />
        } else if (status === ValidationStatus.Valid) {
            return (
                <span className='text-success ml-2'>
                    <i className='fa fa-fw fa-check' /> Validated
                </span>
            )
        } else if (status === ValidationStatus.Failed) {
            const Component = this.getValidationFailed()
            return <Component />
        }
    }

    private getValidationFailed = () =>
        withTooltip('Click to Validate')(props => (
            <button className='btn btn-sm btn-danger' type='button'>
                <i className='fa fa-fw fa-exclamation' />
                Validation Failed
            </button>
        ))

    private getValidationRequired = () =>
        withTooltip('Click to Validate')(props => (
            <button
                className='btn btn-sm btn-warning'
                type='button'
                onClick={this.onValidate}
            >
                <i className='fa fa-fw fa-exclamation-triangle' />
                Validation Required
            </button>
        ))

    private onDelete = async () => {
        const shouldDelete = await show(props => (
            <Delete summoner={this.props.summoner} {...props} />
        ))

        if (shouldDelete) {
            await deleteSummoner(this.props.summoner.id)
            this.setState({ deleted: true })
            setTimeout(() => {
                store.dispatch(actions.removeSummoner(this.props.summoner.id))
            }, 1200)
        }
    }

    private onValidate = async () => {
        await show(props => (
            <Validate summoner={this.props.summoner} {...props} />
        ))
    }
}

interface IButtonProps {
    onClick: VoidFunction
}

const TrashButton = withTooltip('Delete')((props: IButtonProps) => (
    <button
        className='btn btn-sm btn-danger'
        onClick={props.onClick}
        type='button'
    >
        <i className='fa fa-fw fa-trash' />
    </button>
))

export default Card
