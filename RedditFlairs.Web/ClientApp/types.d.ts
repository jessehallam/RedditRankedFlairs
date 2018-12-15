declare namespace app {
    interface IFlair {
        cssText: string;
        id: number;
        needToSend: boolean;
        text: string;
        updatedAt: string;
    }

    interface IProfile {
        flairs: IFlair[];
        id: number;
        name: string;
        summoners: ISummoner[];
    }

    interface ILeaguePosition {
        id: number;
        queueType: string;
        rank: string;
        tier: string;
    }

    interface IValidation {
        attemptedAt: string;
        attempts: number;
        code: string;
        id: number;
        status: number;
    }

    interface ISummoner {
        accountId: string;
        id: number;
        leaguePositions: ILeaguePosition[];
        puuid: string;
        rankUpdatedAt: string;
        region: string;
        summonerId: string;
        summonerName: string;
        validation: IValidation;
    }

    interface IState {
        loaded?: boolean;
        modal?: string;
        profile?: IProfile;
        summoners?: ISummoner[];
    }
}

declare namespace bootstrap {
    interface ITooltipOptions {
        placement?: 'auto' | 'top' | 'bottom' | 'left' | 'right'
        title?: string
    }
}

interface JQuery {
    modal(command: string): JQuery
    tooltip(options: bootstrap.ITooltipOptions): JQuery
}

interface Window {
    __REDUX_DEVTOOLS_EXTENSION__();
    $: JQuery;
}