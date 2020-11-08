
## immersion-mirror

This is a console application which converts video files placed in an input directory into audio files at a designated output directory. It is intended to be used for generating audio content for language learners from native language video files.

## Setup

Docker is the preferred method of setup. The included `docker-compose.yml` contains volume mounts which you can change to customize the input and output directories:
```yaml
volumes:
      - ./VideoInput:/app/InDir
      - ./AudioOutput:/app/OutDir
```