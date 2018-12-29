- step:
    # set GCLOUD_PROJECT environment variable to your project ID
    # set GCLOUD_API_KEYFILE environment variable to base64-encoded keyfile as described here: https://confluence.atlassian.com/x/dm2xNQ
    name: Deploy to GCloud
    deployment: test   # set to test, staging or production
    # trigger: manual  # uncomment to have a manual step
    image: google/cloud-sdk:latest
    script:
      - echo $GCLOUD_API_KEYFILE | base64 --decode --ignore-garbage > ./gcloud-api-key.json
      - gcloud auth activate-service-account --key-file gcloud-api-key.json
      - gcloud config set project $GCLOUD_PROJECT
      - gcloud -q app deploy app.yaml
