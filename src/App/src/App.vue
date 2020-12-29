<template>
  <div :data-theme="selectedThemeKey">
    <div id="options">
      <span id="inverter" @click="onInverterClick"></span>
      <select id="theme-selector" class="custom-select" v-model="selectedTheme">
        <option v-for="theme in themes" :key="theme.key" :value="theme.key">{{ theme.name }}</option>
      </select>
    </div>
    <router-view />
  </div>
</template>

<script lang="ts">
import { Vue } from "vue-class-component";

interface Theme {
  key: string;
  name: string;
}

export default class App extends Vue {
  private _themes!: Theme[]
  public get themes() {
    return this._themes.filter(theme => !theme.key.endsWith('-inverted'))
  }

  private _selectedTheme: Theme = { key: '', name: '' }
  public get selectedTheme() {
    return this._selectedTheme.key
  }
  public set selectedTheme(value) {
    const key = value.replaceAll('-inverted', '')
    this._selectedTheme = this._themes.find(theme => theme.key == key) as Theme
    this.inverted = false;
  }
  public get selectedThemeKey() {
    return this.inverted ? `${this._selectedTheme.key}-inverted` : this._selectedTheme.key
  }

  public inverted = false

  created() {
    this._themes = getComputedStyle(document.documentElement)
                  .getPropertyValue('--theme-names')
                  .split(' ')
                  .filter(x => x != '')
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

  public onInverterClick() {
    this.inverted = !this.inverted
  }
}
</script>

<style lang="scss">
@import url('https://fonts.googleapis.com/css2?family=Lato:wght@100;300;400;700;900&display=swap');

html, body {
  margin: 0;
  padding: 0;
  font-family: 'Lato', sans-serif;
}

#options {
  position: absolute;
  display: flex;
  flex-direction: row;
  align-items: center;
  left: 10px;
  top: 10px;
}

#inverter {
  display: flex;
  $size: 40px;
  height: $size;
  width: $size;
  background-color: var(--foreground);
  border-radius: $size;
}

#theme-selector {
  display: flex;
  padding: 5px 0 10px 10px;
  font-size: 1.4rem;
  background: none;
  text-transform: capitalize;
}
</style>