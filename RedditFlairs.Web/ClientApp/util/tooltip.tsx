import * as React from 'react'
import * as ReactDOM from 'react-dom'

export function withTooltip(title: string) {
    return function withTooltipFactory<TProps>(
        Component: React.ComponentType<TProps>
    ) {
        return class TooltipComponent extends React.Component<TProps> {
            componentDidMount() {
                const node = ReactDOM.findDOMNode(this)
                $(node).tooltip({ title })
            }

            render() {
                return <Component {...this.props} />
            }
        }
    }
}
