export type SubjectCallback<TPayload> = (payload?: TPayload) => void
export type SubjectSubscription = VoidFunction

class Subject<TPayload> {
    private subscriptions: Array<SubjectCallback<TPayload>> = []

    next(payload?: TPayload) {
        this.subscriptions.forEach(callback => callback(payload))
    }

    subscribe(callback: SubjectCallback<TPayload>): SubjectSubscription {
        this.subscriptions.push(callback)
        return () => this.unsubscribe(callback)
    }

    private unsubscribe(callback: SubjectCallback<TPayload>) {
        const i = this.subscriptions.indexOf(callback)
        if (i >= 0) this.subscriptions.splice(i, 1)
    }
}

export default Subject
