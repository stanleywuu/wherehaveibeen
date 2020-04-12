export default {
  getUserAuth (state) {
    return state.userAuth !== undefined
  },
  getUserToken (state) {
    return state.userAuth
  },
  getUserId (state) {
    return state.userId
  },
  getUserStatus (state){
    return state.userStatus
 }
}