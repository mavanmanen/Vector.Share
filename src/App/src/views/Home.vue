<template>
  <div>
    <div id="container">
      <div id="upload-form" class="box">
        <input id="file" type="file" @change="onFileChange" />
        <label for="file">
          <span>
            {{ boxText }}
            <span>{{ sizeText }}</span>
          </span>
          

          <select v-model="model.lifetime" class="custom-select">
            <option :value="lifetimeOptions.D1">1 day</option>
            <option :value="lifetimeOptions.D7">1 week</option>
            <option :value="lifetimeOptions.M1">1 month</option>
          </select>
          <button type="button" @click="uploadOnClick">Upload</button>
        </label>
      </div>
      <div id="progress-overlay" class="box" :class="progressClass">
        <a href="" @click="copyLink">Click to copy link</a>
        <input id="hiddenUrl" ref="hiddenUrl" type="text" v-model="uploadedUrl" />
      </div>
    </div>

    <footer>Whatever you upload is your own responsibility, I don't fucking care.</footer>
  </div>
</template>

<script lang="ts">
import { Vue } from 'vue-class-component'
import ApiService from '../api-service'
import { eventBus } from '../main';
import { FileLifetime, UploadModel } from '../models'

export default class Home extends Vue {
  public model: UploadModel = {
    lifetime: FileLifetime.D1,
    file: null
  }

  public progressClass = this.makeProgressClass(0)

  private readonly _defaultBoxText = 'Drop or select a file'
  public boxText = this._defaultBoxText

  private readonly _defaultSizeText = '1 GiB max'
  public sizeText = this._defaultSizeText

  public uploadedUrl = ''

  public get lifetimeOptions(): typeof FileLifetime {
    return FileLifetime
  }

  private api: ApiService = new ApiService()

  mounted() {
    eventBus.on('onUploadProgress', (percentCompleted) => this.updateProgress(percentCompleted))
  }

  onFileChange(e: Event) {
    const files = (e.target as HTMLInputElement).files

    if(files?.length == 1) {
      const file = files[0]
      const fileSizeKib = file.size/1024
      const fileSizeMiB = fileSizeKib/1024
      const fileSizeGiB = fileSizeMiB/1024

      if(fileSizeGiB <= 1) {
        this.model.file = file
        this.boxText = file.name
        
        if(fileSizeKib < 1024) {
          this.sizeText = `${fileSizeKib.toFixed(2)} KiB / 1 GiB`
        } else if(fileSizeMiB < 1024) {
          this.sizeText = `${fileSizeMiB.toFixed(2)} MiB / 1 GiB`
        }

      } else {
        this.reset()
      }
    }
  }

  async uploadOnClick() {
    const result = await this.api.uploadAsync(this.model)
    if(result == null) {
      return
    }

    this.uploadedUrl = result.url
  }

  updateProgress(percentCompleted: number) {
    this.progressClass = this.makeProgressClass(percentCompleted)
  }

  makeProgressClass(value: number) {
    return `progress-${value}`
  }

  copyLink(event: Event) {
    event.preventDefault()
    const urlField = this.$refs.hiddenUrl as HTMLInputElement
    urlField.select()
    document.execCommand('copy')
    this.reset()
  }

  reset() {
    this.model = {
      lifetime: FileLifetime.D1,
      file: null
    }
    this.progressClass = this.makeProgressClass(0)
    this.boxText = this._defaultBoxText
    this.sizeText = this._defaultSizeText
    this.uploadedUrl = ''
  }
}
</script>

<style lang="scss">
$footer-height: 40px;
$border: solid 2px var(--foreground);
$box-size: 450px;
$border-radius: $box-size / 30;

#container{
  display: flex;
  align-items: center;
  justify-content: center;
  height: calc(100vh - (#{$footer-height} + 2px));
  width: 100vw;
  color: var(--foreground);
  background-color: var(--background);

  .box {
    height: $box-size;
    width: $box-size;
    border-radius: $border-radius;
    border: $border;
    position: absolute;
  }

  #upload-form {
    input[type="file"] {
      opacity: 0;
      width: 0;
      height: 0;
      position: absolute;
    }

    label {
      height: $box-size;
      width: $box-size;
      display: flex;
      align-items: center;
      justify-content: center;
      flex-direction: column;
      font-size: 2rem;
      text-align: center;

      * {
        width: 100%;
      }

      & > span {
        display: flex;
        flex-grow: 1;
        justify-content: center;
        align-items: center;
        flex-direction: column;
        
        span {
          font-size: .8em;
          margin-top: 15px;
        }
      }

      select {
        height: $box-size / 6;
        border-top: $border;
        border-bottom: $border;
        background-size: 40px auto, 100%;
        text-align-last: center;
      }

      button {
        height: $box-size / 6;
        border-radius: 0 0 $border-radius $border-radius;
        border: none;
        background: transparent;
        font-size: inherit;
        font-family: inherit;
        color: inherit;

        &:focus {
          outline: none;
        }
      }      
    }
  }

  #progress-overlay {
    pointer-events: none;
    display: flex;
    justify-content: center;
    align-items: center;
    font-size: 2rem;

    &.progress-100 a {
        visibility: visible;
    }

    a {
      visibility: hidden;;
      pointer-events: all;
      color: var(--background);
      text-decoration: none;
    }

    #hiddenUrl {
      position: absolute;
      opacity: 0;
    }
  }
}

footer {
  border-top: $border;
  height: $footer-height;
  display: flex;
  justify-content: center;
  justify-items: center;
  width: 100%;
  line-height: $footer-height;
  font-size: 1.2rem;
  color: var(--foreground);
  background-color: var(--background);
}
</style>
