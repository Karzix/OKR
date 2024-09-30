<template>
    <div class="common-layout">
        <router-view />
    </div>
    
</template>
<style scoped>
.common-layout {
  background-image: url("../../assets/web.png");
  background-repeat: no-repeat;
  background-size: cover; /* Đảm bảo hình nền phủ toàn bộ khung */
  background-position: center center;
  background-attachment: fixed; /* Cố định hình nền */
  min-height: 100vh; /* Chiều cao tối thiểu luôn bằng chiều cao của màn hình */
  width: 100%; /* Đảm bảo chiều rộng luôn bằng chiều rộng của màn hình */
}
</style>