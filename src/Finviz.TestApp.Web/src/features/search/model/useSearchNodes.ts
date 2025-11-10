import { useQuery } from "@tanstack/react-query";
import { searchNodes } from "@/entities/imagenet/api/searchApi";
import { useDebounce } from "@/shared/lib/useDebounce";

export const useSearchNodes = (rawQuery: string) => {
    const query = useDebounce(rawQuery, 300);

    return useQuery({
        queryKey: ["search", query],
        queryFn: () => searchNodes(query),
        enabled: query.trim().length > 0
    });
};
