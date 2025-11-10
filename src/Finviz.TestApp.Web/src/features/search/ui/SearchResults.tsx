import {
    useCallback,
    useEffect,
    useState,
    memo
} from "react";
import {
    List,
    ListItemButton,
    ListItemText,
    Typography,
    Pagination,
    Box
} from "@mui/material";
import { useSearchNodes } from "../model/useSearchNodes";
import { highlightText } from "@/shared/lib/highlightText";
import { SEARCH_PAGE_SIZE } from "@/shared/config/constants";
import { useTheme } from "@mui/material/styles";
import type { SearchResponse } from "@/entities/imagenet/api/searchApi";

interface Props {
    query: string;
    onSelect: (id: number) => void;
}

export const SearchResults = memo(({ query, onSelect }: Props) => {
    const [page, setPage] = useState(1);

    const theme = useTheme();

    useEffect(() => {
        setPage(1);
    }, [query]);

    const { data, isFetching, isError } = useSearchNodes(query, page, SEARCH_PAGE_SIZE) as {
        data: SearchResponse | undefined;
        isFetching: boolean;
        isError: boolean;
    };

    const handleSelect = useCallback((id: number) => onSelect(id), [onSelect]);

    if (!query.trim()) return null;

    if (isError)
        return <Typography color="error">Failed to load results.</Typography>;

    if (isFetching && !data)
        return <Typography variant="body2">Searching...</Typography>;

    if (!data || data.items.length === 0)
        return <Typography variant="body2">Nothing found</Typography>;

    const totalPages = Math.max(1, Math.ceil(data.total / SEARCH_PAGE_SIZE));

    return (
        <Box display="flex" flexDirection="column" height="100%">
            <Box flex="1 1 auto" overflow="auto">
                <List dense>
                    {data.items.map((item) => (
                        <ListItemButton key={item.id} onClick={() => handleSelect(item.id)}>
                            <ListItemText
                                primary={highlightText(item.name, query, theme)}
                                secondary={highlightText(item.fullPath, query, theme)}
                            />
                        </ListItemButton>
                    ))}
                </List>
            </Box>

            <Box
                flex="0 0 auto"
                display="flex"
                flexDirection="column"
                alignItems="center"
                justifyContent="center"
                mt={1}
            >
                <Typography
                    variant="caption"
                    sx={{
                        mb: 1,
                        textAlign: "center",
                        color: "text.secondary",
                    }}
                >
                    {(() => {
                        const start = (page - 1) * SEARCH_PAGE_SIZE + 1;
                        const end = Math.min(page * SEARCH_PAGE_SIZE, data.total);
                        return `${start}–${end} of ${data.total} items`;
                    })()}
                </Typography>

                <Pagination
                    count={totalPages}
                    page={page}
                    onChange={(_, value) => setPage(value)}
                    size="small"
                    color="primary"
                />
            </Box>
        </Box>
    );
});
