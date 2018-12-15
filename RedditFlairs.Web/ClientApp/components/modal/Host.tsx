import * as React from 'react'

import Subject, { SubjectSubscription } from '../../util/subject'

import { IModalProps, IRenderProps, RenderFunction } from '.'
import Context, { IContext } from './Context'
import Modal from './Modal'

interface IModalHostProps {}
interface IModalHostState {
    modalProps: IModalProps
    resolve: (result: any) => void
    value: null
}

interface IShowProps {
    render: RenderFunction
    resolve: (result: any) => void
}

const showSubject = new Subject<IShowProps>()

class ModalHost extends React.Component<IModalHostProps, IModalHostState> {
    readonly state: IModalHostState = {
        modalProps: null,
        resolve: null,
        value: null
    }

    private contextArg: IContext = {
        setValue: (value: any) => this.setState({ value })
    }
    private subscription: SubjectSubscription

    componentDidMount() {
        this.subscription = showSubject.subscribe(this.onShow)
    }
    componentWillUnmount() {
        this.subscription()
    }

    render() {
        if (!this.state.modalProps) return null
        return (
            <Context.Provider value={this.contextArg}>
                <Modal {...this.state.modalProps} />
            </Context.Provider>
        )
    }

    private onClosed = () => {
        if (this.state.resolve) {
            this.state.resolve(this.state.value)
        }
        this.setState({
            modalProps: null,
            resolve: null,
            value: null
        })
    }

    private onShow = (props: IShowProps) => {
        this.setState({
            modalProps: {
                closed: this.onClosed,
                render: props.render
            },
            resolve: props.resolve
        })
    }
}

export function show<TResult>(render: RenderFunction): Promise<TResult> {
    return new Promise<TResult>(resolve => {
        showSubject.next({ render, resolve })
    })
}

export default ModalHost
