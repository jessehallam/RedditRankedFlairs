import * as React from 'react'

import { IModalProps, IRenderProps } from '.'
import Context, { IContext } from './Context'
import { IModalState } from './Modal'

interface IModalPortalProps extends IModalProps, IModalState {}
interface IModalPortalPropsActual extends IModalPortalProps {
    context: IContext
}

class ModalPortal extends React.Component<IModalPortalPropsActual> {
    private node: HTMLDivElement
    private renderProps: IRenderProps = {
        cancel: () => this.closeWithValue(undefined),
        confirm: (value?: any) => this.closeWithValue(value)
    }

    componentDidMount() {
        console.log('context =', this.props.context)
        $(this.node).on('hidden.bs.modal', () => {
            this.props.closed(undefined)
        })
    }

    componentDidUpdate(prevProps) {
        if (!prevProps.isOpen && this.props.isOpen) {
            $(this.node).modal('show')
        }
    }

    render() {
        return (
            <div
                className='modal fade'
                tabIndex={-1}
                role='dialog'
                ref={div => (this.node = div)}
            >
                <div className='modal-dialog' role='document'>
                    <div className='modal-content'>
                        {this.props.render(this.renderProps)}
                    </div>
                </div>
            </div>
        )
    }

    private closeWithValue = (value: any) => {
        this.props.context.setValue(value)
        $(this.node).modal('hide')
    }
}

export default function ModalPortalWithContext(props: IModalPortalProps) {
    return (
        <Context.Consumer>
            {context => <ModalPortal context={context} {...props} />}
        </Context.Consumer>
    )
}

// export default ModalPortal
