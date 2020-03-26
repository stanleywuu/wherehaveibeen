<template>
  <div id="app">
    <b-card id='login-form-card' title="Login">
      <LoginForm></LoginForm>
    </b-card>
  </div>
</template>

<script>
import LoginForm from '@/components/LoginForm.vue'

export default {
  name: 'App',
  components: {
    LoginForm
  }
}
</script>

<style>
#app {
  font-family: Avenir, Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
  margin-top: 60px;
}

#login-form-card {
  width: 50%;
  margin: auto;
}
</style>
