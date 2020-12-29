module.exports = {
  css: {
    loaderOptions: {
      sass: {
        prependData : `
          @use "@/style/_includes.scss";
        `
      }
    }
  }
};