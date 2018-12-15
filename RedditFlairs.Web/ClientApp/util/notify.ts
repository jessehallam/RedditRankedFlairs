import * as Noty from 'noty'

import 'noty/lib/noty.css'
import 'noty/lib/themes/mint.css'

export const raiseNotification = (options: Noty.Options) =>
    new Noty(options).show()

export const success = (message: string) =>
    raiseNotification({ text: message, type: 'success', timeout: 3000 })