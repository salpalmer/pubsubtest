# pub

Repository for reproducing an apparent breakpoint issue in VSCode Kubernetes remote attach debugging.
<br><br>

# Dependencies

<p>This code has been executed in docker on a Apple Mac with ARM chip.  The functionality should be compaitble with distro that has Docker installed.</p>
<p>

## Dependencies for the full demonstration include:
 - VSCode
 - dapr
 - docker

 # Demonstration

 ## Setup
  1. In a terminal window navigate to the root directory of extracted source.
  2. Start docker
  
 ## Exercising the demo - working with ttl config in 1.12.5
  1. In terminal execute: docker-compose -f docker_compose-ttl-working.yml up
  2. Once all containers are running execute the following curl command to initiate the demo
    curl -X GET http://localhost:50340/Start
  3. Open docker logs for the pub, sub services and pub-dapr-1, sub-dapr-1 sidecars. 
   You should see in the logs:
   - the pub container log report "Command Published to DaprPubSub:pubsub DaprTopic:service-command-bus"
   - the pub-dapr-1 container log report: 
      msg=msg="Processing Redis message 1710491322448-0" app_id=pub component="pubsub (pubsub.redis)" instance=655026a02abb scope=dapr.contrib type=log ver=1.12.5
      msg=msg="no matching route for event <nil> in pubsub pubsub and topic service-command-bus; skipping" app_id=pub instance=655026a02abb scope=dapr.runtime.processor.pubsub type=log ver=1.12.5
   - the sub-dapr-1 container log report:
      msg="Processing Redis message 1710491290084-0" app_id=sub component="pubsub (pubsub.redis)" instance=304e53bf8563 scope=dapr.contrib type=log ver=1.12.5  
   - the sub container report "/CreateAbstractExample Endpoint Triggered"

## Exercising the demo - failure with ttl config in 1.13.0 
  1. In terminal execute: docker-compose -f docker_compose-ttl-failure.yml up
  2. Once all containers are running execute the following curl command to initiate the demo
    curl -X GET http://localhost:50340/Start
  You should see in the logs:
   - the pub container log report "Command Published to DaprPubSub:pubsub DaprTopic:service-command-bus"
   - the pub-dapr-1 container log report: 
      msg="Processing Redis message 1710490759233-0" app_id=pub component="pubsub (pubsub.redis)" instance=d798dd769bbc scope=dapr.contrib type=log ver=1.13.0
      msg="dropping expired pub/sub event <nil> as of 2024-03-15T08:19:19Z" app_id=pub instance=d798dd769bbc scope=dapr.runtime.processor.pubsub type=log ver=1.13.0
   - the sub-dapr-1 container log report:
      msg="Processing Redis message 1710490759233-0" app_id=sub component="pubsub (pubsub.redis)" instance=2e5dd53642ee scope=dapr.contrib type=log ver=1.13.0
      msg="dropping expired pub/sub event <nil> as of 2024-03-15T08:19:19Z" app_id=sub instance=2e5dd53642ee scope=dapr.runtime.processor.pubsub type=log ver=1.13.0   
   - the sub container never log it received and handled the message

