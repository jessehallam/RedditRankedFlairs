FROM    microsoft/dotnet:2.2-sdk as build
WORKDIR /build

ENV NODE_VERSION 10.14.2
ENV NODE_DOWNLOAD_SHA 0552b0f6fc9c0cd078bbb794c876e2546ba63a1dfcf8e3c206387936696ca128

RUN curl -SL "https://nodejs.org/dist/v${NODE_VERSION}/node-v${NODE_VERSION}-linux-x64.tar.gz" --output nodejs.tar.gz \
    && echo "$NODE_DOWNLOAD_SHA nodejs.tar.gz" | sha256sum -c - \
    && tar -xzf "nodejs.tar.gz" -C /usr/local --strip-components=1 \
    && rm nodejs.tar.gz \
    && ln -s /usr/local/bin/node /usr/local/bin/nodejs

COPY    RedditFlairs.sln ./
COPY    RedditFlairs.Web/RedditFlairs.Web.csproj ./RedditFlairs.Web/
COPY    RedditFlairs.Core/RedditFlairs.Core.csproj ./RedditFlairs.Core/

RUN     dotnet restore

COPY    . ./
RUN     dotnet publish -c Release -o ../publish_output

FROM    microsoft/dotnet:2.2-aspnetcore-runtime
WORKDIR /app
COPY    --from=build /build/publish_output .

ENV     ASPNETCORE_ENVIRONMENT Production
ENV     ASPNETCORE_URLS=http://+:80
