<template>
  <div :data-theme="selectedThemeKey">
    <div id="theme-selector">
      <span @click="onInverterClick"></span>
      <select class="custom-select" v-model="selectedTheme">
        <option v-for="theme in themes" :key="theme.key" :value="theme.key">{{ theme.name }}</option>
      </select>
    </div>

    <div id="container">
      <div id="upload-form" class="box">
        <input id="file" type="file" ref="file-input" @change="onFileChange" />
        <label for="file" @dragover="onUploadFormDragOver" @drop="onUploadFormDrop">
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

      <div id="progress-overlay" ref="progress-overlay" @click="copyLink" class="box overlay" :class="progressClass">
        <span ref="progress-overlay-text">Click to copy link</span>
        <input id="hidden-url" ref="hidden-url" type="text" v-model="uploadedUrl" />
      </div>

      <div id="error-overlay" ref="error-overlay" class="box overlay" @click="errorOverlayOnClick">
        <span ref="error-overlay-text">{{ errorMessage }}</span>
      </div>
    </div>
    <footer>Whatever you upload is your own responsibility, I don't fucking care.</footer>
  </div>
</template>

<script lang="ts">
import { Vue } from 'vue-class-component'
import ApiService from '@/api-service'
import { eventBus } from '@/main'
import { FileLifetime, UploadModel } from '@/models'

interface Theme {
  key: string;
  name: string;
}

export default class App extends Vue {
  private _themes!: Theme[]
  private _selectedTheme: Theme = { key: '', name: '' }
  private readonly defaultSizeText = '1 GiB max'
  private readonly defaultBoxText = 'Drop or select a file'
  private api: ApiService = new ApiService()
  private progressOverlay!: HTMLElement
  private progressOverlayText!: HTMLElement

  public get themes() {
    return this._themes.filter(theme => !theme.key.endsWith('-inverted'))
  }

  public get selectedTheme() {
    return this._selectedTheme.key
  }

  public set selectedTheme(value) {
    const key = value.replaceAll('-inverted', '')
    this._selectedTheme = this._themes.find(theme => theme.key == key) as Theme
    this.inverted = false
  }

  public get selectedThemeKey() {
    return this.inverted ? `${this._selectedTheme.key}-inverted` : this._selectedTheme.key
  }

  public get lifetimeOptions(): typeof FileLifetime {
    return FileLifetime
  }

  public model: UploadModel = {
    lifetime: FileLifetime.D1,
    file: null
  }

  public inverted = false
  public progressClass = this.makeProgressClass(0)
  public boxText = this.defaultBoxText
  public sizeText = this.defaultSizeText
  public errorMessage = ''
  public uploadedUrl = ''

  public created() {
    this._themes = getComputedStyle(document.documentElement)
                  .getPropertyValue('--theme-names')
                  .trim()
                  .split(' ')
                  .map(theme => {
                    return {
                      key: theme,
                      name: theme.replaceAll('--', ' & ').replaceAll('-', ' ')
                    } as Theme
                  })
                  .sort((a, b) => a.name.localeCompare(b.name))

    const themeIndex = Math.floor(Math.random() * this._themes.length)
    const theme = this._themes[themeIndex]

    this._selectedTheme = theme
  }

  public mounted() {
    this.progressOverlay = this.getElementByRef('progress-overlay')
    this.progressOverlayText = this.getElementByRef('progress-overlay-text')
    eventBus.on('onUploadProgress', (percentCompleted) => this.updateProgress(percentCompleted))
  }

  public onInverterClick = () => this.inverted = !this.inverted
  public onFileChange = (e: Event) => this.loadFile((e.target as HTMLInputElement).files)

  public onUploadFormDragOver(event: DragEvent) {
    event.stopPropagation()
    event.preventDefault()
    if(event.dataTransfer != null) {
      event.dataTransfer.dropEffect = 'copy'
    }
  }

  public onUploadFormDrop(event: DragEvent) {
    event.stopPropagation()
    event.preventDefault()
    if(event.dataTransfer != null) {
      this.loadFile(event.dataTransfer.files)
    }
  }

  private loadFile(files: FileList | null) {
    if(files == null) {
      return
    }

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
        this.showErrorOverlay(`File too large:\n${fileSizeGiB.toFixed(2)} GiB / 1 GiB`)
        this.reset()
      }
    }
  }

  private showErrorOverlay(text: string) {
    const errorOverlay = this.getElementByRef('error-overlay')
    this.errorMessage = text
    errorOverlay.style.visibility = 'visible'
  }

  public errorOverlayOnClick() {
    const errorOverlay = this.getElementByRef('error-overlay')
    this.errorMessage = ''
    errorOverlay.style.visibility = 'hidden'
  }

  private getElementByRef(ref: string) {
    return this.$refs[ref] as HTMLElement
  }

  public async uploadOnClick() {
    this.progressOverlay.style.cursor = 'progress'
    this.progressOverlay.style.visibility = 'visible'

    const result = await this.api.uploadAsync(this.model)

    if(result == null) {
      return
    }

    this.uploadedUrl = result.url
    this.progressOverlay.style.cursor = 'pointer'
    this.progressOverlayText.style.visibility = 'visible'
  }

  private updateProgress(percentCompleted: number) {
    this.progressClass = this.makeProgressClass(percentCompleted)
  }

  private makeProgressClass(value: number) {
    return `progress-${value}`
  }

  private copyLink(event: Event) {
    event.stopPropagation()
    event.preventDefault()

    const urlField = this.getElementByRef('hidden-url') as HTMLInputElement
    urlField.select()
    document.execCommand('copy')
    this.reset()
  }

  private reset() {
    this.model = {
      lifetime: FileLifetime.D1,
      file: null
    }
    this.progressClass = this.makeProgressClass(0)
    this.progressOverlay.style.visibility = 'hidden'
    this.progressOverlay.style.cursor = 'none'
    this.progressOverlayText.style.visibility = 'hidden'
    this.boxText = this.defaultBoxText
    this.sizeText = this.defaultSizeText
    this.uploadedUrl = ''
    const fileInput = this.getElementByRef('file-input') as HTMLInputElement
    fileInput.files = null
  }
}
</script>

<style lang="scss">
@use '@/style/_themes';
@use '@/style/_progress';
@import url('https://fonts.googleapis.com/css2?family=Lato:wght@100;300;400;700;900&display=swap');

$footer-height: 40px;
$border: solid 2px var(--foreground);
$box-size: 450px;
$border-radius: $box-size / 30;

html, body {
  margin: 0;
  padding: 0;
  font-family: 'Lato', sans-serif;
}

select.custom-select {
  border: none;
  background-color: transparent;
  font-size: inherit;
  font-family: inherit;
  color: var(--foreground);
  -moz-appearance: none;
  -webkit-appearance: none;
  appearance: none;
  background-image: var(--dropdown-arrow);
  background-repeat: no-repeat, repeat;
  background-position: right 20px top 50%, 0 0;
  background-size: 10px auto, 100%;

  &:focus {
    outline: none;
  }

  option {
    background-color: var(--background);
  }
}

#container {
  display: flex;
  align-items: center;
  justify-content: center;
  height: calc(100vh - (#{$footer-height} + 2px));
  width: 100vw;
  color: var(--foreground);
  background-color: var(--background);
}

#theme-selector {
  position: absolute;
  display: flex;
  flex-direction: row;
  align-items: center;
  left: 10px;
  top: 10px;

  & > span {
    display: flex;
    $size: 40px;
    height: $size;
    width: $size;
    background-color: var(--foreground);
    border-radius: $size;
    cursor: pointer;
  }

  & > select {
    display: flex;
    padding: 5px 0 10px 10px;
    font-size: 1.4rem;
    background: none;
    text-transform: capitalize;
  }
}

.box {
  height: $box-size;
  width: $box-size;
  border-radius: $border-radius;
  border: $border;
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
    height: $box-size;
    width: $box-size;
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
      height: $box-size / 6;
    }

    select {
      border-top: $border;
      border-bottom: $border;
      background-size: 40px auto, 100%;
      text-align-last: center;
    }

    button {
      border-radius: 0 0 $border-radius $border-radius;
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

.overlay {
  visibility: hidden;
  display: flex;
  justify-content: center;
  align-items: center;
  font-size: 2rem;
  background-color: var(--foreground);

  span {
    color: var(--background);
    visibility: visible;
  }
}

#progress-overlay {
  cursor: none;
  background: none;

  span {
    visibility: hidden;
    pointer-events: all;
    cursor: inherit;
  }

  #hidden-url {
    z-index: -99999;
    position: absolute;
    opacity: 0;
    cursor: inherit;
  }
}

#error-overlay {
  cursor: pointer;
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