import * as React from 'react'
import * as ReactDOM from 'react-dom'

import { IModalProps } from '.'

import Portal from './Portal'

export interface IModalState {
    isOpen?: boolean
}

class Modal extends React.Component<IModalProps, IModalState> {
    private node: HTMLDivElement

    constructor(props) {
        super(props)
        this.node = document.createElement('div')
        this.state = {}
    }

    componentDidMount() {
        document.body.appendChild(this.node)
        this.setState({ isOpen: true })
    }

    componentWillUnmount() {
        document.body.removeChild(this.node)
    }

    render() {
        return ReactDOM.createPortal(
            <Portal isOpen={this.state.isOpen} {...this.props} />,
            this.node
        )
    }

}

export default Modal