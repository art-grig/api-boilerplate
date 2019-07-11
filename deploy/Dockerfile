FROM microsoft/dotnet:2.2-aspnetcore-runtime
RUN apt-get update && apt-get -y install jq

WORKDIR /app
EXPOSE 80

COPY ./app /app
COPY ./deploy /deploy

ENTRYPOINT ["/bin/bash", "/deploy/start.sh"]
