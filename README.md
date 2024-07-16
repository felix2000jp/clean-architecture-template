# Clean Architecture Template

## Configuration

This project has two main configuration files, the **launchSettings.json** file - used to run the application manually - 
and the **compose.yml** (the environment variables for the service *notes*) - used for running tests. The settings in 
both files should be exactly the same, except for network related variables (example: database connection string) due to
the way docker containers' networking works.

## CI/CD

This template makes use of GitHub actions for its CI/CD pipeline.

Every commit 1 GitHub app and 1 GitHub workflow (with 5 jobs), making 6 status checks in total, will run against your
changes and only if every check is successful, will you be able to merge your changes. Here is a rundown of those
checks.

### GitHub Workflow - Build and Test

This workflow is made of 5 different jobs and its intent is to build and test the application with your changes applied.

#### Build builder image

This job is the **FIRST** to run on the workflow.

It first runs the resharper code inspection tool against the code. If no issues were found the job keeps running, else
the pipeline fails. In case of failure, you can see what issues were found in the output of the
**Install and run inspectcode** step.

It then builds the builder stage of the docker file and pushes the image (through the push option) and layer cache
(through the cache-to option) to the GitHub packages registry, so that they can be used in the following jobs to improve
performance.

**NOTE:** Even though we have implemented both a cache (layer caching) and a build context (using already built images),
if you take a look at the workflow summary you will see that the cache is never used. This happens because we cached
the layers and built the image for the builder stage, because we are reusing the image we never hit the cache. I am
leaving it like this so projects derived from this template can use the method that makes more sense for their use case.

#### Run unit tests

This job is dependent on [Build builder image](#build-builder-image) and runs in parallel
with [Run integration tests](#run-integration-tests).

It builds the tests-unit stage making use of the already built builder image (through the build-contexts options) and
the layer cache (through the cache-from option) pushed to GitHub packages registry
in [Build builder image](#build-builder-image).

#### Run integration tests

This job is dependent on [Build builder image](#build-builder-image) and runs in parallel
with [Run unit tests](#run-unit-tests).

It builds the tests-integration stage making use of the already built builder image (through the build-contexts options)
and the layer cache (through the cache-from option) pushed to GitHub packages registry
in [Build builder image](#build-builder-image).

#### Build final image

This job is dependent on [Run unit tests](#run-unit-tests) and [Run integration tests](#run-integration-tests). It only
runs on the main branch to avoid
cluttering the docker registry with images for every commit on feature branches.

It builds the final docker image - the image to use in production - making use of the already built builder image
(through the build-contexts options) and the layer cache (through the cache-from option) pushed to GitHub packages
registry in [Build builder image](#build-builder-image). After building the image it pushes it to the docker registry.

#### Clean up ghcr

This job is dependent on [Build final image](#build-final-image) and runs everytime, even if one of the other steps
failed.

It cleans up the GitHub packages registry deleting any unnecessary images (It's configured to have only 10 images at a
time and to delete any untagged images)

### GitHub Action - Semantic PRs

It verifies your Pull Request title follows the conventional commit guidelines. For more information on the rules being
enforced, take a look at
the [angular commit message guidelines](https://github.com/angular/angular/blob/22b96b9/CONTRIBUTING.md#-commit-message-guidelines).

- build: Changes that affect the build system or external dependencies (example scopes: gulp, broccoli, npm)
- ci: Changes to our CI configuration files and scripts (example scopes: Travis, Circle, BrowserStack, SauceLabs)
- docs: Documentation only changes
- feat: A new feature
- fix: A bug fix
- perf: A code change that improves performance
- refactor: A code change that neither fixes a bug nor adds a feature
- style: Changes that do not affect the meaning of the code (white-space, formatting, missing semicolons, etc...)
- test: Adding missing tests or correcting existing tests
- revert: Revert existing code
- chore: Changes that don't modify the source code directly but are important for maintaining the project
