import { createContext } from 'react'

export interface IContext {
    setValue(value: any)
}

export default createContext<IContext>(null)