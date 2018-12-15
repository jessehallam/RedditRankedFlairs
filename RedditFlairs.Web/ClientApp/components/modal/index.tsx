import * as React from 'react'

import Host, { show } from './Host'
import Modal from './Modal'

export type RenderFunction = (props: IRenderProps) => JSX.Element

export interface IModalProps {
    closed: (result: any) => void
    render: RenderFunction
}

export interface IRenderProps {
    cancel: VoidFunction
    confirm: (value?: any) => void
}

export { show }
export default Host

export function CloseButton() {
    return (
        <button
            type='button'
            className='close'
            data-dismiss='modal'
            aria-label='Close'
        >
            <span aria-hidden='true'>&times;</span>
        </button>
    )
}
