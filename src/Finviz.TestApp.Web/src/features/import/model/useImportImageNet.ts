import { useMutation } from "@tanstack/react-query";
import { importImageNet } from "@/entities/imagenet/api/importApi";

export const useImportImageNet = () => {
    return useMutation({
        mutationFn: importImageNet,
    });
};
