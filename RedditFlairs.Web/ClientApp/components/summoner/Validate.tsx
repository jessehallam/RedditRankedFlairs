import * as React from 'react'

import { CloseButton, IRenderProps } from '../modal'

interface IValidateProps extends IRenderProps {
    summoner: app.ISummoner
}

class Validate extends React.Component<IValidateProps> {
    render() {
        return (
            <div>
                <div className='modal-header'>
                    <h5 className='modal-title'>Validate Summoner</h5>
                    <CloseButton />
                </div>
                <div className='modal-body'>
                    <p>
                        To validate{' '}
                        <strong>
                            {this.props.summoner.summonerName} (
                            {this.props.summoner.region})
                        </strong>
                        , set your{' '}
                        <a href='#' target='_blank'>
                            third-party validation code
                        </a>{' '}
                        to the following:
                    </p>
                    <blockquote>{this.props.summoner.validation.code}</blockquote>
                    <p>
                        Once your code is updated, validation will typically happen in an hour or two.
                    </p>
                </div>
            </div>
        )
    }
}

export default Validate
