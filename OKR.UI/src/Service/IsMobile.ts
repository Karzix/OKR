import {ref} from "vue";
import {onMounted, onUnmounted} from "vue";

export const isMobile = ref(false);
const handleResize = () => {
  isMobile.value = window.innerWidth < 600;
    console.log("a",isMobile.value);
};

onMounted(() => {
  window.addEventListener("resize", handleResize);
  handleResize(); // Gọi hàm này để kiểm tra kích thước ban đầu
});

onUnmounted(() => {
  window.removeEventListener("resize", handleResize);
});
