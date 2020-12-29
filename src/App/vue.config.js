module.exports = {
  css: {
    loaderOptions: {
      sass: {
        prependData : `
          @use '@/style/_colors';
          @use '@/style/_variables';
          @use '@/style/_themes';
          @use '@/style/_progress';
          @use '@/style/_inputs';
        `
      }
    }
  }
}