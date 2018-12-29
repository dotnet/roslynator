- step:
    # set AWS_ACCESS_KEY_ID, AWS_SECRET_ACCESS_KEY and AWS_DEFAULT_REGION as environment variables
    name: Deploy to AWS
    deployment: test   # set to test, staging or production
    # trigger: manual  # uncomment to have a manual step
    image: atlassian/pipelines-awscli
    script:
      - aws deploy push --application-name <application-name> --s3-location s3://<S3 bucket>/<s3-key> --ignore-hidden-files
      - aws deploy create-deployment --application-name <application-name> --s3-location bucket=<s3-bucket>,key=<s3-key>,bundleType=zip --deployment-group-name <deployment-group>
