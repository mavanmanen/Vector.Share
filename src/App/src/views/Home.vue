<template>
  <div>
    <div id="upload-form" class="box">
      <input id="file" type="file" @change="onFileChange" />
      <label for="file">
        <span>
          <span>{{ boxText }}</span>
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

    <div id="progress-overlay" ref="progress-overlay" @click="copyLink" class="box" :class="progressClass">
      <span >Click to copy link</span>
      <input id="hidden-url" ref="hidden-url" type="text" v-model="uploadedUrl" />
    </div>
  </div>
</template>

<script lang="ts">
import { Vue } from 'vue-class-component'
import ApiService from '../api-service'
import { eventBus } from '../main'
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
    const overlay = this.$refs['progress-overlay'] as HTMLElement
    overlay.style.visibility = 'visible'

    const result = await this.api.uploadAsync(this.model)

    if(result == null) {
      return
    }

    this.uploadedUrl = result.url
    overlay.style.cursor = 'pointer'
  }

  updateProgress(percentCompleted: number) {
    this.progressClass = this.makeProgressClass(percentCompleted)
  }

  makeProgressClass(value: number) {
    return `progress-${value}`
  }

  copyLink(event: Event) {
    event.preventDefault()
    const urlField = this.$refs['hidden-url'] as HTMLInputElement
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
    const overlay = this.$refs['progress-overlay'] as HTMLElement
    overlay.style.visibility = 'hidden'
    overlay.style.cursor = 'progress'
    this.boxText = this._defaultBoxText
    this.sizeText = this._defaultSizeText
    this.uploadedUrl = ''
  }
}
</script>

<style lang="scss">
.box {
  height: variables.$box-size;
  width: variables.$box-size;
  border-radius: variables.$border-radius;
  border: variables.$border;
  position: absolute;
}

#upload-form {
  #file {
    opacity: 0;
    width: 0;
    height: 0;
    position: absolute;
  }

  label {
    height: variables.$box-size;
    width: variables.$box-size;
    display: flex;
    align-items: center;
    justify-content: center;
    flex-direction: column;
    font-size: 2rem;
    text-align: center;
    cursor: pointer;

    & > span {
      display: flex;
      flex-grow: 1;
      justify-content: center;
      align-items: center;
      flex-direction: column;
      overflow-wrap: anywhere;
      
      & > span {
        padding: 0 20px;

        &:nth-child(2) {
          margin-top: 15px;
          font-size: .8em;
        }
      }
    }

    select, button {
      width: 100%;
      height: variables.$box-size / 6;
    }

    select {
      border-top: variables.$border;
      border-bottom: variables.$border;
      background-size: 40px auto, 100%;
      text-align-last: center;
    }

    button {
      border-radius: 0 0 variables.$border-radius variables.$border-radius;
      border: none;
      background: transparent;
      font-size: inherit;
      font-family: inherit;
      color: inherit;
      cursor: pointer;

      &:focus {
        outline: none;
      }
    }      
  }
}

#progress-overlay {
  visibility: hidden;
  display: flex;
  justify-content: center;
  align-items: center;
  font-size: 2rem;
  cursor: progress;

  &.progress-100 span {
      visibility: visible;
  }

  span {
    visibility: hidden;
    pointer-events: all;
    color: var(--background);
    text-decoration: none;
    cursor: inherit;
  }

  #hidden-url {
    position: absolute;
    opacity: 0;
    cursor: inherit;
  }
}
</style>
