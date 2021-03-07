# Transformalize AWS Providers / Transforms

This repository houses AWS related things for Transformalize.

## Provider(s)

### logs describe-log-groups

This pulls all the log groups.

```xml
<cfg name='test'>
   <connections>
      <add name='input' provider='aws' service='logs' command='DescribeLogGroups' />
   </connections>
   <entities>
      <add name='entity'>
         <fields>
            <add name='arn' primary-key='true' />
            <add name='creationTime' type='datetime' />
            <add name='kmsKeyId' length='256' />
            <add name='logGroupName' length='512' />
            <add name='metricFilterCount' type='int' />
            <add name='retentionInDays' type='int' />
            <add name='storedBytes' type='long' />
         </fields>
      </add>
   </entities>
</cfg>
```

## Transform(s)

### sts get-caller-identity

Currently this is a transform that returns 
a JSON response from the AWS STS (Secure Token Service) 
to get the caller's identity.  You use it as a transform like this:

`t="stsgetcalleridentity()"`
